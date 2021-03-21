<template>
    <div>
        <b-form-group :id="'fieldset-' + id" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <b-form-textarea v-bind="$attrs" v-model="inputModel" :formatter="inputFormatter" @keyup="$emit('update:model', inputModel);" @blur="$emit('change');"></b-form-textarea>
                <b-form-checkbox v-if="showModelExact" :id="'checkbox-' + id"  v-model="inputModelExact" @change="$emit('update:modelExact', inputModelExact)" :name="'checkbox-' + id" value="1" unchecked-value="0" >
                    <span class="col-form-label">exact</span>
                </b-form-checkbox>
                <b-form-invalid-feedback :id="'input-' + id + '-live-feedback'" :state="state">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                {{ inputModel }}
            </div>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["model", "modelExact", "errorMessage", "id", "state", "isEditable", "change", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            inputModel:this.model,
            inputModelExact:this.modelExact,
            showModelExact:this.modelExact ? true : false,
        }
    }, 
    methods: {
        inputFormatter(value) {
            return value;
        }
    },
    computed : {
        isAllowEdit : {
            get: function() {
                if (this.isEditable != undefined) return this.isEditable;
                else return true;
            }
        }
    },
    watch: {
        model(newValue) {
            this.inputModel = newValue
        },
        modelExact(newValue) {
            this.inputModelExact = newValue
        }
    }
}
</script>