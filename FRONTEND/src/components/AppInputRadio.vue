<template>
    <div>
        <b-form-group v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <b-form-radio-group v-model="inputModel" v-bind="$attrs" :options="options" :state="state" @change="dataChange">
                    <b-form-invalid-feedback :state="state">{{ errorMessage }}</b-form-invalid-feedback>
                </b-form-radio-group>
            </div>
            <div v-else>
                {{ inputModel }}
            </div>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["model", "errorMessage", "state", "options", "isEditable", "change", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            inputModel:this.model,
        }
    },
    methods: {
        dataChange() {
            this.$emit('update:model', this.inputModel);
            this.$emit("change");
        },
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
        }
    }
}
</script>