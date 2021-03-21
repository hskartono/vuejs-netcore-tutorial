<template>
    <div>
        <b-form-group :id="'fieldset-' + id" boundary="viewport" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <b-form-datepicker v-bind="$attrs"  boundary="viewport" class="boundary-datepicker" v-model="inputModel" @input="setDate"></b-form-datepicker>
                <b-form-invalid-feedback :id="'input-' + id + '-live-feedback'" :state="state">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                {{ inputModel | moment('DD-MMM-YYYY')}}
            </div>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["model", "id", "errorMessage", "state", "suffix", "prefix", "isEditable", "change", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            inputModel:this.model,
            prefixExist: (this.prefix ? true : false),
            suffixExist: (this.suffix ? true : false),
        }
    }, 
    methods: {
        setDate() {
            this.$emit('update:model', this.inputModel);
            this.$emit('change')
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
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>